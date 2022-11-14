import imp
import re

from glob import glob

test_types = {}
test_type_folders = glob("/MyBenchmark/toolset/test_types/*/")

for folder in test_type_folders:
    test_type_name = re.findall(r'.+\/(.+)\/$', folder, re.M)[0]
    test_type = imp.load_source("TestType", "%s%s.py" % (folder, test_type_name))
    test_types[test_type_name] = test_type.TestType
