#!/usr/bin/env python3
import re

def analyze_prefab(filepath):
    with open(filepath, 'r') as f:
        content = f.read()
    
    # 提取所有GameObject
    objects = []
    pattern = r'--- !u!1 &([0-9]+)\s+GameObject:.*?m_Name:\s*(.+)'
    matches = re.findall(pattern, content, re.DOTALL)
    
    for match in matches:
        obj_id = match[0]
        name_match = re.search(r'm_Name:\s*(.+)', match[1])
        if name_match:
            name = name_match.group(1).strip()
            if name:  # 只记录非空名称
                objects.append((obj_id, name))
    
    return objects

# 分析WordCard
wordcard_objects = analyze_prefab('Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/WordCard.prefab')
print("=== WordCard预制体节点结构 ===")
for i, (obj_id, name) in enumerate(wordcard_objects, 1):
    print(f"{i}. {name} (ID: {obj_id})")
print(f"\n总计: {len(wordcard_objects)} 个节点")

# 分析CategorySlot
categoryslot_objects = analyze_prefab('Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/CategorySlot.prefab')
print("\n=== CategorySlot预制体节点结构 ===")
for i, (obj_id, name) in enumerate(categoryslot_objects, 1):
    print(f"{i}. {name} (ID: {obj_id})")
print(f"\n总计: {len(categoryslot_objects)} 个节点")
