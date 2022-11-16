import json

res_filename = '../results/20221116083216/results.json'
with open(res_filename) as res_f:
    data = json.load(res_f)
for x in data.items():
    print(x)
