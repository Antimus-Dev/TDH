 // app.js is the main entry point for your 8th Wall app. Code here will execute after head.html
// is loaded, and before body.html is loaded.
import {ARFunctions} from './AR_Functions'
AFRAME.registerComponent('image-target-card', ARFunctions())

import {modelOpacityComponent} from './model-opacity'
AFRAME.registerComponent('model-opacity', modelOpacityComponent)