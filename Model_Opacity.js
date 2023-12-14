 const modelOpacityComponent = {
  schema: {default: 1.0},
  init() {
    this.el.addEventListener('model-loaded', this.update.bind(this))
  },
  update() {
    const mesh = this.el.getObject3D('mesh')
    const data = this.data

    if (!mesh) { return }
    mesh.traverse((node) => {
       if (node.isMesh) {
        node.material.opacity = data
        node.material.transparent = data < 1.0
        node.material.needsUpdate = true
       }
    })
  },
}
export {modelOpacityComponent}