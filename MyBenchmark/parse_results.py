import json
import os
from prettytable import PrettyTable
from run_combination import N_REPEATS
import re
from openpyxl import Workbook
import csv
import pandas as pd
pd.options.display.width = 0
#N_REPEATS = 1

res_dir = '/results/'
res_name = '/results.json'
res_filenames = list(map(lambda sub_dir: res_dir[1:] + sub_dir + res_name, os.listdir(os.getcwd() + res_dir)[-N_REPEATS * 2 - 1:]))
res_filenames.remove('results/excel/results.json')
#print(res_filenames)

first_res_filename = res_filenames[0]
with open(first_res_filename) as res_f:
    data = json.load(res_f)
# for x in data.items():
#     print(x)




queryLevels = data['queryLevels']
jsonConcurrencyLevels = data['jsonConcurrencyLevels']
plaintextConcurrencyLevels = data['plaintextConcurrencyLevels']

levels = {'plaintext': {'value': plaintextConcurrencyLevels, 'name': 'plaintextConcurrencyLevels'},
          'query': {'value': queryLevels, 'name': 'queryLevels'},
          'json': {'value': jsonConcurrencyLevels, 'name': 'jsonConcurrencyLevels'},
          }
lengths = {name: len(levels[name]['value']) for name in levels}

fails = []

results_sum = {
    fw_name: {
        tes_name:
            [{'totalRequests': 0, 'totalSeconds': 0, 'totalAvg': 0, 'totalLatencyAvg': 0, 'maxLatencyMax': 0}
             for _ in range(lengths[tes_name])]
        for tes_name in ['plaintext', 'query', 'json']
    } for fw_name in ['aiohttp', 'crax']
}

for filename in res_filenames:
    with open(filename) as res_f:
        data = json.load(res_f)
    fails.append(data['failed'])

    fw_name = data['frameworks'][0]
    test_results = data['rawData']
    for test_name, _test_results in test_results.items():
        test_results = _test_results[fw_name]
        # print(test_name, test_results)
        for i, test_result in enumerate(test_results):
            results_sum[fw_name][test_name][i]['totalRequests'] += test_result['totalRequests']
            results_sum[fw_name][test_name][i]['totalSeconds'] += (test_result['endTime'] - test_result['startTime'])
            results_sum[fw_name][test_name][i]['totalAvg'] += (test_result['totalRequests'] /
                                                               (test_result['endTime'] - test_result['startTime']))
            results_sum[fw_name][test_name][i]['totalLatencyAvg'] += float(test_result['latencyAvg'][:-2])
            results_sum[fw_name][test_name][i]['maxLatencyMax'] = max(float(test_result['latencyMax'][:-2]),
                                                                      results_sum[fw_name][test_name][i]['maxLatencyMax'])


results_final = {
    fw_name: {
        tes_name:
            [{
                'totalRequests': results_sum[fw_name][tes_name][i]['totalRequests'] / N_REPEATS,
                'seconds': results_sum[fw_name][tes_name][i]['totalSeconds'] / N_REPEATS,  # дб 15
                'requestsPerSecond': results_sum[fw_name][tes_name][i]['totalAvg'] / N_REPEATS,
                'latencyAvg': results_sum[fw_name][tes_name][i]['totalLatencyAvg'] / N_REPEATS,
                'latencyMax': results_sum[fw_name][tes_name][i]['maxLatencyMax'] / N_REPEATS
            } for i in range(lengths[tes_name])]
        for tes_name in ['plaintext', 'query', 'json']
    } for fw_name in ['aiohttp', 'crax']
}



# for fw_name, res_fw in results_final.items():
#     print()
#     print(fw_name)
#
#     for k, v in res_fw.items():
#         print(k, v)




for test_name in ['plaintext', 'query', 'json']:
    res_table = PrettyTable()
    res_table.field_names = ["framework", levels[test_name]['name'], 'totalRequests', 'seconds', 'requestsPerSecond', 'latencyAvg', 'latencyMax']

    for fw_name in ['aiohttp', 'crax']:
        for i, level_value in enumerate(levels[test_name]['value']):
            res_table.add_row([fw_name, level_value,
                               results_final[fw_name][test_name][i]['totalRequests'],
                               results_final[fw_name][test_name][i]['seconds'],
                               results_final[fw_name][test_name][i]['requestsPerSecond'],
                               results_final[fw_name][test_name][i]['latencyAvg'],
                               results_final[fw_name][test_name][i]['latencyMax'],
                               ])

    #print(res_table)
    tbl_as_csv = res_table.get_csv_string().replace('\r', '')
    csv_filename = res_dir[1:] + 'excel/' + f"{test_name}_from_{re.findall(r'/(.*?)/', res_filenames[0])[0]}" \
                                 f"_to_{re.findall(r'/(.*?)/', res_filenames[-1])[0]}.csv"
    with open(csv_filename, "w") as text_file:
        text_file.write(tbl_as_csv)

    df = pd.read_csv(csv_filename)
    df.to_excel(f'{csv_filename[:-4]}.xlsx')

    print()
    print(test_name)
    print('Sorted by requestsPerSecond')
    df = df.sort_values(by='requestsPerSecond', ascending=False)
    print(df)
    print('Sorted by latencyAvg')
    df = df.sort_values(by='latencyAvg', ascending=True)
    print(df)

    # wb = Workbook()
    # ws = wb.active
    # with open(csv_filename, 'r') as f:
    #     for row in csv.reader(f):
    #         ws.append(row)
    # wb.save(f'{csv_filename[:-4]}.xlsx')


print(fails)

