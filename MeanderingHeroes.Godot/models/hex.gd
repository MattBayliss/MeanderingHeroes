class_name Hex

var q:int
var r:int

func _init(q:= 0, r:= 0):
	q = q
	r = r
	
func from_offset(v: Vector2i):
	q = v.x - (v.y - (v.y&1)) / 2
	r = v.y
	
func _to_string() -> String:
	return "(" + str(q) + ", " + str(r) + ")"	
