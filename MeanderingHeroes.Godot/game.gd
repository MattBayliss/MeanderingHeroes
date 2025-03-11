extends Node2D

@onready var tile_map: TileMapLayer = $TileMapLayer


func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.pressed:
		if event.button_index == MOUSE_BUTTON_LEFT :
			var offset = tile_map.local_to_map(event.position)
		
			var hex = Hex.new()
			hex.from_offset(tile_map.local_to_map(event.position))
			
		# this is offset odd-r hex coordinates, need to convert to axial q,r
			print(str(offset) + " :: " + hex.to_string())
