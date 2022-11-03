#!/usr/bin/env tarantool


io_module = require("io")
io_module.stdout:setvbuf("no")
math_module = require("math")
json=require('json')
msgpack=require('msgpack')
json_data_dir = "/usr/local/share/tarantool/json_data/"


------------------------------------------------------------------------------------------------fill tables
local function load_users_data()
    local cur_space = box.space.users
	local cur_filename = "users.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["user_id"], v["card_id"], v["user_email"], v["password"], v["permissions"]}
    end
end

local function load_turnstiles_data()
    local cur_space = box.space.turnstiles
	local cur_filename = "turnstiles.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["turnstile_id"], v["lift_id"], v["is_open"]}
    end
end

local function load_lifts_data()
    local cur_space = box.space.lifts
	local cur_filename = "lifts.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["lift_id"], v["lift_name"], v["is_open"], v["seats_amount"], v["lifting_time"], v["queue_time"]}
    end
end


local function load_slopes_data()
    local cur_space = box.space.slopes
	local cur_filename = "slopes.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["slope_id"], v["slope_name"], v["is_open"], v["difficulty_level"]}
    end
end

local function load_lifts_slopes_data()
    local cur_space = box.space.lifts_slopes
	local cur_filename = "lifts_slopes.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["record_id"], v["lift_id"], v["slope_id"]}
    end
end


local function load_data()
	load_users_data()
	load_turnstiles_data()
	load_lifts_data()
	load_slopes_data()
	load_lifts_slopes_data()
end

box.cfg {
}
load_data()

