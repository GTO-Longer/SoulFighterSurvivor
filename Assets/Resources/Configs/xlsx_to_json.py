import os
import pandas as pd
import json

def convert_xlsx_to_json_in_current_dir():
    """
    将当前目录下所有 .xlsx 文件转换为同名 .json 文件。
    - 读取第一个工作表。
    - 第一列为属性名（如 'id'），必须包含一行名为 'hero_name' 的行用于获取英雄名。
    - 后续每列为一个英雄的数据。
    - 输出格式：{ "heroes": [ { "heroName": "...", ... }, ... ] }
    """
    current_dir = os.path.dirname(os.path.abspath(__file__))
    xlsx_files = [f for f in os.listdir(current_dir) if f.lower().endswith('.xlsx')]

    if not xlsx_files:
        print("⚠️ 当前目录下没有找到 .xlsx 文件。")
        return

    for filename in xlsx_files:
        xlsx_path = os.path.join(current_dir, filename)
        json_filename = os.path.splitext(filename)[0] + '.json'
        json_path = os.path.join(current_dir, json_filename)

        try:
            # 读取 Excel，保持字符串类型避免自动转 float/int 导致 NaN 问题
            df = pd.read_excel(xlsx_path, sheet_name=0, dtype=str)
            df = df.where(pd.notnull(df), None)  # 将 NaN 替换为 None（JSON 中为 null）

            columns = df.columns.tolist()
            if len(columns) < 2:
                print(f"❌ 跳过 {filename}：至少需要两列（属性列 + 至少一个英雄列）")
                continue

            attr_col = columns[0]      # 第一列是属性标识，如 'id'
            hero_cols = columns[1:]    # 其余列是英雄数据

            # 转为字典列表以便按行查找
            records = df.to_dict(orient='records')
            attr_to_row = {}
            for row in records:
                key = row[attr_col]
                if key is not None:
                    attr_to_row[str(key).strip()] = row

            # 检查是否存在 hero_name 行
            if 'hero_name' not in attr_to_row:
                print(f"❌ 跳过 {filename}：缺少 'hero_name' 行（用于提取英雄名称）")
                continue

            hero_name_row = attr_to_row['hero_name']

            # 构建英雄列表
            heroes = []
            for col in hero_cols:
                # 获取英雄名：优先用 hero_name 行的值，否则用列名
                raw_name = hero_name_row.get(col)
                hero_name = str(raw_name).strip() if raw_name is not None else col.strip()

                hero_data = {"heroName": hero_name}

                # 遍历所有属性行（跳过 hero_name）
                for attr, row in attr_to_row.items():
                    if attr == 'hero_name':
                        continue

                    value = row.get(col)

                    # 类型智能转换
                    if value is None or (isinstance(value, str) and value.strip() == ''):
                        converted = None
                    elif isinstance(value, str):
                        s = value.strip()
                        # 判断是否为数字（支持负数和小数）
                        if s.replace('.', '', 1).replace('-', '', 1).isdigit() and s.count('-') <= 1 and s.count('.') <= 1:
                            if '.' in s:
                                converted = float(s)
                            else:
                                converted = int(s)
                        else:
                            converted = s
                    else:
                        converted = value

                    hero_data[attr] = converted

                heroes.append(hero_data)

            # 最终输出结构：{ "heroes": [...] }
            output_json = {"heroes": heroes}

            # 写入 JSON 文件
            with open(json_path, 'w', encoding='utf-8') as f:
                json.dump(output_json, f, indent=4, ensure_ascii=False)

            print(f"✅ 已转换: {filename} → {json_filename}")

        except Exception as e:
            print(f"❌ 转换失败: {filename} - 错误: {e}")

    print("🎉 所有 .xlsx 文件处理完成！")

if __name__ == "__main__":
    convert_xlsx_to_json_in_current_dir()
