import os, sys, re, json
from more_itertools import split_when

'''
{
   "levels":[
      {
         "subLevels":[
            {
               "fileName":"level_0"
            },
            {
               "fileName":"level_1"
            }
         ]
      }
   ]
}
'''

# generate levelconfig.json

ret = []
for i in os.listdir("."):
    match = re.match(r"(\d+)-(\d+).json", i)
    if match:
        ret.append([int(match[1]), int(match[2])])

ret.sort()

levels = []
for items in split_when(ret, lambda x, y: x[0] != y[0]):
   levelIndex = items[0][0]
   subLevelIndices = [i[1] for i in items]
   subLevelIndices.sort()
   subLevels = [{"fileName": f"{levelIndex}-{si}"} for si in subLevelIndices]
   levels.append({"subLevels": subLevels})

config = {"levels": levels}

import shutil

if os.path.exists("levels_plain"):
   shutil.rmtree("levels_plain", True)
os.mkdir("levels_plain")

for i in ret:
    fn = f"{i[0]}-{i[1]}.json"
    shutil.copy2(fn, "levels_plain/" + fn)

with open("levels_plain/levelconfig.json", "w") as f:
    f.write(json.dumps(config))

import subprocess

subprocess.check_call("Encrypt levels_plain levels", shell=True)

print("generate levels.zip")

# update meta

with open("meta_plain.json", "r") as f:
   o = json.load(f)

o["version"] += 1

with open("meta_plain.json", "w") as f:
   json.dump(o, f)

import encrypt

encrypt.EncryptFile("meta_plain.json", "meta.json")

print("generate meta.json")

# upload

import subprocess

s3Folder = "s3://public-ce19f4f2-a8cf-4210-8209-82b441412ee0/SpringMatch/"

subprocess.check_call(f"aws s3 cp levels.zip {s3Folder} --acl public-read", shell=True)
subprocess.check_call(f"aws s3 cp meta.json {s3Folder} --acl public-read", shell=True)

print("upload all files")

ans = input("exit...")