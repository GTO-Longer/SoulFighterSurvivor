import os
import pandas as pd
import json

def convert_xlsx_to_json_in_current_dir():
    current_dir = os.path.dirname(os.path.abspath(__file__))
    xlsx_files = [f for f in os.listdir(current_dir) if f.lower().endswith('.xlsx')]
    if not xlsx_files:
        print("当前目录下没有找到 .xlsx 文件。")
        return

    for filename in xlsx_files:
        xlsx_path = os.path.join(current_dir, filename)
        json_filename = os.path.splitext(filename)[0] + '.json'
        json_path = os.path.join(current_dir, json_filename)

        try:
            df = pd.read_excel(xlsx_path, sheet_name=0, dtype=str)
            df = df.where(pd.notnull(df), None)
            columns = df.columns.tolist()
            if len(columns) < 2:
                print(f"跳过 {filename}：至少需要两列")
                continue

            attr_col = columns[0]
            data_cols = columns[1:]  # 技能英文名或英雄ID
            records = df.to_dict(orient='records')
            attr_to_row = {}
            for row in records:
                key = row[attr_col]
                if key is not None:
                    attr_to_row[str(key).strip()] = row

            # 判断配置类型
            is_skill_config = 'skillName' in attr_to_row or '_skillType' in attr_to_row
            is_hero_config = 'heroName' in attr_to_row

            if is_skill_config:
                # === 修改后的 SkillConfig 处理逻辑：输出为 { "skills": [ {...}, ... ] } 格式 ===
                skills_list = []
                for col in data_cols:  # col 是技能英文ID，如 "EssenceTheft"
                    skill_data = {"id": col}  # 添加 id 字段
                    for attr, row in attr_to_row.items():
                        value = row.get(col)
                        if attr == '_baseSkillValue':
                            # 新格式："[40,65,90],[40,65,90]" 或空值
                            if value is None or str(value).strip() == '':
                                skill_data[attr] = []
                            else:
                                v_str = str(value).strip()
                                if v_str.startswith('[') and v_str.endswith(']'):
                                    inner = v_str[1:-1]
                                else:
                                    inner = v_str
                                if not inner:
                                    skill_data[attr] = []
                                else:
                                    parts = inner.split('],[')
                                    converted_2d = []
                                    for part in parts:
                                        clean_part = part.strip('[]')
                                        if not clean_part:
                                            nums = []
                                        else:
                                            nums = [x.strip() for x in clean_part.split(',') if x.strip()]
                                        try:
                                            int_list = [float(x) for x in nums]
                                            converted_2d.append(int_list)
                                        except Exception:
                                            converted_2d.append([])
                                    skill_data[attr] = converted_2d
                        elif attr in ['_baseSkillCost', '_baseSkillCoolDown']:
                            if value is None or str(value).strip() == '':
                                skill_data[attr] = []
                            else:
                                try:
                                    v_str = str(value).strip()
                                    if v_str.startswith('[') and v_str.endswith(']'):
                                        v_str = v_str[1:-1]
                                    nums = [x.strip() for x in v_str.split(',') if x.strip()]
                                    skill_data[attr] = [int(float(x)) for x in nums]
                                except Exception:
                                    skill_data[attr] = []
                        elif attr in ['_skillBulletType', '_skillUsageType']:
                            if value is None or str(value).strip() == '':
                                skill_data[attr] = []
                            else:
                                skill_data[attr] = [x.strip() for x in str(value).split(',') if x.strip()]
                        elif attr == '_skillRange':
                            try:
                                skill_data[attr] = int(float(value)) if value is not None else 0
                            except:
                                skill_data[attr] = 0
                        else:
                            skill_data[attr] = smart_convert(value)
                    skills_list.append(skill_data)
                output_json = {"skills": skills_list}

            elif is_hero_config:
                # === 保留原有 HeroConfig 处理逻辑 ===
                heroes = []
                for col in data_cols:
                    raw_name = attr_to_row['heroName'].get(col)
                    hero_name = str(raw_name).strip() if raw_name is not None else col.strip()
                    hero_data = {"heroName": hero_name}
                    for attr, row in attr_to_row.items():
                        if attr == 'heroName':
                            continue
                        value = row.get(col)
                        converted = smart_convert(value)
                        hero_data[attr] = converted
                    heroes.append(hero_data)
                output_json = {"heroes": heroes}
            else:
                print(f"跳过 {filename}：无法识别配置类型（缺少 heroName 或 skillName）")
                continue

            with open(json_path, 'w', encoding='utf-8') as f:
                json.dump(output_json, f, indent=4, ensure_ascii=False)
            print(f"已转换: {filename} → {json_filename}")

        except Exception as e:
            print(f"转换失败: {filename} - 错误: {e}")

    print("所有 .xlsx 文件处理完成！")

def smart_convert(value):
    """智能转换字符串为 int/float/str/None"""
    if value is None or (isinstance(value, str) and value.strip() == ''):
        return None
    if isinstance(value, str):
        s = value.strip()
        if s.replace('.', '', 1).replace('-', '', 1).isdigit() and s.count('-') <= 1 and s.count('.') <= 1:
            if '.' in s:
                return float(s)
            else:
                return int(s)
        else:
            return s
    else:
        return value

if __name__ == "__main__":
    convert_xlsx_to_json_in_current_dir()
