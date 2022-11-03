#!/usr/bin/env tarantool


io_module = require("io")
io_module.stdout:setvbuf("no")
math_module = require("math")
json=require('json')
msgpack=require('msgpack')
json_data_dir = "/usr/local/share/tarantool/json_data/"


local function rollback_users_data()
    local cur_space = box.space.users
	for row in cur_space do
		print(row)
	end
	
	
	local cur_filename = "users.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["user_id"], v["card_id"], v["user_email"], v["password"], v["permissions"]}
    end
	
	
	for row in cur_space do
		print(row)
	end
end


rollback_users_data()
--load_data()
print('end of app.init')

