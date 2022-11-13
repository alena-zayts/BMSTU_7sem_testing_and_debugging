import imp

db_name = 'postgres'
db = imp.load_source("Database", "/FrameworkBenchmarks/toolset/databases/postgres/postgres.py")
databases = {db_name: db.Database}

