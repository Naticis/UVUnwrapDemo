import bpy

bpy.ops.import_scene.obj(filepath = 'export.obj')
bpy.ops.object.editmode_toggle()
bpy.ops.uv.smart_project(angle_limit=0.767945)
bpy.ops.export_scene.obj(filepath="output.obj", use_selection=True)