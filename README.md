# SimpleFlyCamera

A simple fly camera for Unity3D.

## Features

- Play mode quit on escape
- Object select and focus
- Camera FOV adjustment
- Camera zoom
- Camera rotation
- Camera drag movement
- Camera movement
- Camera speed shift
- Camera speed adjustment
- Reset camera position and rotation
  - Click on the `Reset Camera` button in the inspector component context menu

## Requirements

- Unity 2022.3.6f1 or higher

## Installation

Just simply drag and drop the prefab `Assets/RekornTools/SimpleFlyCamera/SimpleFlyCamera.prefab` into your scene.

## Uninstallation

Just simply delete the prefab `SimpleFlyCamera.prefab` from your scene.

## Configuration

You can configure the camera by changing the values of the `SimpleFlyCamera` component.

## Controls

### General

- `Esc` - Quit application

### Camera

- `Space` - Reset camera position and rotation

### Selection & Focus

- `LMB` - Select object under mouse cursor
- `LCtrl` + `LMB` - Select object under mouse cursor + Select object in hierarchy
- `F` - Move camera to selected object

### FOV

- `LCtrl` + `Wheel Up` - Decrease camera FOV
- `LCtrl` + `Wheel Down` - Increase camera FOV
- `R` - Reset camera FOV

### Zoom

- `Wheel Up` - Zoom camera in
- `Wheel Down` - Zoom camera out

### Rotation

- `RMB` + `Mouse X` - Rotate camera yaw
- `RMB` + `Mouse Y` - Rotate camera pitch
- `LAlt` + `RMB` + `Mouse X` - Rotate camera roll

### Drag

- `MMB` + `Mouse X` - Drag camera left/right
- `MMB` + `Mouse Y` - Drag camera up/down
- `LAlt` + `MMB` + `Mouse X` - Drag camera left/right
- `LAlt` + `MMB` + `Mouse Y` - Drag camera forward/backward

### Movement

- `RMB` + `W` - Move camera forward
- `RMB` + `S` - Move camera backward
- `RMB` + `A` - Move camera left
- `RMB` + `D` - Move camera right
- `RMB` + `Q` - Move camera down
- `RMB` + `E` - Move camera up
- `RMB` + `LShift` - Shift camera movement speed to faster
- `RMB` + `LControl` - Shift camera movement speed to slower
- `RMB` + `Wheel Up` - Adjust camera movement speed to faster
- `RMB` + `Wheel Down` - Adjust camera movement speed to slower

## Contributing

Feel free to contribute to this project by submitting issues and/or pull requests.

## License

MIT License

## Author

Rekorn