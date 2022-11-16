from toolset.test_types.abstract_test_type import AbstractTestType


class TestType(AbstractTestType):
    def __init__(self, config):
        self.json_url = ""
        kwargs = {'name': 'json', 'accept_header': self.what_accepts('json'),
                  'requires_db': False, 'args': ['json_url']}
        AbstractTestType.__init__(self, config, **kwargs)

    def get_url(self):
        return self.json_url

    def get_script_name(self):
        return 'concurrency.sh'

    def get_script_variables(self, name, url):
        return {
            'max_concurrency': max(self.config.json_concurrency_levels),
            'name': name,
            'duration': self.config.duration,
            'levels': " ".join("{}".format(item) for item in self.config.json_concurrency_levels),
            'server_host': self.config.server_host,
            'url': url,
            'accept': "application/json,text/html;q=0.9,application/xhtml+xml;q=0.9,application/xml;q=0.8,*/*;q=0.7"
        }
