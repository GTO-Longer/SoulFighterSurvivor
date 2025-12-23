import os
import pandas as pd
import json

def smart_convert(value):
    """智能转换字符串为 int/float/str/None"""
    if value is None or (isinstance(value, str) and value.strip() == ''):
        return None
    if isinstance(value, str):
        s = value.strip()
        # 尝试解析 JSON（用于 _attributeList 等字段）
        if s.startswith('{') and s.endswith('}'):
            try:
                return json.loads(s)
            except Exception:
                pass  # 如果不是合法 JSON，继续按普通字符串处理
        if s.replace('.', '', 1).replace('-', '', 1).isdigit() and s.count('-') <= 1 and s.count('.') <= 1:
            if '.' in s:
                return float(s)
            else:
                return int(s)
        else:
            return s
    else:
        return value

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
            data_cols = columns[1:]
            records = df.to_dict(orient='records')
            attr_to_row = {}
            for row in records:
                key = row[attr_col]
                if key is not None:
                    attr_to_row[str(key).strip()] = row

            # 判断配置类型
            is_skill_config = 'skillName' in attr_to_row or '_skillType' in attr_to_row
            is_hero_config = 'heroName' in attr_to_row
            is_equipment_config = 'equipmentName' in attr_to_row
            is_hex_config = 'hexName' in attr_to_row

            if is_skill_config:
                # === SkillConfig 处理逻辑（保持不变）===
                skills_list = []
                for col in data_cols:
                    skill_data = {"id": col}
                    for attr, row in attr_to_row.items():
                        value = row.get(col)
                        if attr == '_baseSkillValue':
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
                # === HeroConfig 处理逻辑（保持不变）===
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

            elif is_equipment_config:
                # === EquipmentConfig 处理逻辑 ===
                equipments = []
                for col in data_cols:
                    equipment_data = {"id": col}  # 以列名（英文ID）作为 id
                    for attr, row in attr_to_row.items():
                        value = row.get(col)
                        
                        # --- 修改开始：针对 _usageType 进行特殊处理 ---
                        if attr == '_usageType':
                            if value is None or str(value).strip() == '':
                                equipment_data[attr] = []
                            else:
                                # 按逗号分割字符串，去除空格，生成列表
                                equipment_data[attr] = [x.strip() for x in str(value).split(',') if x.strip()]
                        # --- 修改结束 ---
                        
                        else:
                            converted = smart_convert(value)
                            equipment_data[attr] = converted
                            
                    equipments.append(equipment_data)
                output_json = {"equipments": equipments}

            elif is_hex_config:
                # === HexConfig 处理逻辑 ===
                hexes = []
                for col in data_cols:
                    hex_data = {"id": col}
                    for attr, row in attr_to_row.items():
                        value = row.get(col)
                        converted = smart_convert(value)
                        hex_data[attr] = converted
                    hexes.append(hex_data)
                output_json = {"hexes": hexes}

            else:
                print(f"跳过 {filename}：无法识别配置类型（缺少 heroName / skillName / equipmentName）")
                continue

            with open(json_path, 'w', encoding='utf-8') as f:
                json.dump(output_json, f, indent=4, ensure_ascii=False)
            print(f"已转换: {filename} → {json_filename}")

        except Exception as e:
            print(f"转换失败: {filename} - 错误: {e}")

    print("所有 .xlsx 文件处理完成！")

if __name__ == "__main__":
    convert_xlsx_to_json_in_current_dir()
