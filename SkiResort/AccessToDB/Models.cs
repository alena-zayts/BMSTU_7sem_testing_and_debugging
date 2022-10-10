
global using UserDB = System.ValueTuple<uint, uint, string, string, uint>;
global using LiftDB = System.ValueTuple<uint, string, bool, uint, uint, uint>;
global using LiftSlopeDB = System.ValueTuple<uint, uint, uint>;
global using SlopeDB = System.ValueTuple<uint, string, bool, uint>;
global using TurnstileDB = System.ValueTuple<uint, uint, bool>;


global using UserDBNoIndex = System.ValueTuple<uint, string, string, uint>;
global using LiftDBNoIndex = System.ValueTuple<string, bool, uint, uint, uint>;
global using LiftSlopeDBNoIndex = System.ValueTuple<uint, uint>;
global using SlopeDBNoIndex = System.ValueTuple<string, bool, uint>;
global using TurnstileDBNoIndex = System.ValueTuple<uint, bool>;

