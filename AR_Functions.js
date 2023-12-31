 let visible = true

function PlaySound(asset) {
  const sound = new window.Howl({
    src: [require(`./assets/${asset}.mp3`)],
  })
  sound.play()
}

const ARFunctions = () => ({
  schema: {
    name: {type: 'string'},
  },
  init() {
    const {object3D} = this.el
    const {name} = this.data
    const mainScene = document.getElementById('mainScene')
    object3D.visible = false

    // selects the objects
    const mountain = this.el.sceneEl.querySelector('#mountainEntity')
    const topo = this.el.sceneEl.querySelector('#mountainTopography')
    const enterEntity = this.el.sceneEl.querySelector('#enter')
    const sound = this.el.sceneEl.querySelector('#sfx-shimmer')
    const buttonAudio = document.getElementById('buttonAudio')

    const showImage = ({detail}) => {
      if (name !== detail.name) {
        return
      }
      object3D.position.copy(detail.position)
      object3D.quaternion.copy(detail.rotation)
      object3D.scale.set(detail.scale, detail.scale, detail.scale)
      object3D.visible = true
      visible = true
    }

    const imageFound = (e) => {
      if (visible) {
// Animations
// {
        object3D.visible = true
        mountain.setAttribute('model-opacity', 0)

        // Topo, Mountain, and Web Button Animation
        topo.setAttribute('visible', true)
        topo.setAttribute('animation__height', {
          'property': 'gltf-morph.value',
          'from': 1,
          'to': 0,
          'dur': 4800,
          'dir': 'normal',
          'easing': 'easeInOutQuad',
        })

        mountain.setAttribute('animation__height', {
          'property': 'gltf-morph.value',
          'from': 1,
          'to': 0,
          'dur': 5000,
          'dir': 'normal',
          'easing': 'easeInOutQuad',
        })
        enterEntity.setAttribute('animation__position', {
          'property': 'position',
          'from': '-0.04 0.1 0.4',
          'to': '-0.04 0.1 0.9',
          'dur': 4500,
          'dir': 'normal',
          'easing': 'easeInOutQuad',
        })

        // Mountain & Web Button Fade In
        setTimeout(() => {
          PlaySound(sound.getAttribute('id'))
          mountain.setAttribute('visible', true)
          mountain.setAttribute('animation__alpha', {
            'property': 'model-opacity',
            'from': 0,
            'to': 1,
            'dur': 1500,
            'dir': 'normal',
            'easing': 'easeInOutQuad',
          })
          enterEntity.setAttribute('visible', true)
          enterEntity.setAttribute('animation__alpha', {
            'property': 'model-opacity',
            'from': 0,
            'to': 0.9,
            'dur': 500,
            'dir': 'normal',
            'easing': 'easeInOutQuad',
          })
        }, 1500)

        // Topo Fade Out
        setTimeout(() => {
          topo.setAttribute('animation__alpha', {
            'property': 'model-opacity',
            'from': 1,
            'to': 0,
            'dur': 1000,
            'dir': 'normal',
            'easing': 'easeInOutQuad',
          })
        }, 2500)
        PlaySound(buttonAudio.getAttribute('id'))
// }
      }
      showImage(e)
    }

    const imageLost = (e) => {
      object3D.visible = false
      visible = false
      setTimeout(() => {
        if (visible) {
          object3D.visible = true
          return
        }
        visible = false
        object3D.visible = false
        topo.setAttribute('gltf-morph', {'value': 1})
        topo.setAttribute('model-opacity', 1)
        topo.removeAttribute('animation__height')
        topo.removeAttribute('animation__alpha')

        enterEntity.setAttribute('visible', false)
        enterEntity.removeAttribute('animation__alpha')
        enterEntity.removeAttribute('animation__position')
        enterEntity.setAttribute('position', '-0.04 0.1 0.4')
        enterEntity.setAttribute('model-opacity', 0)

        mountain.setAttribute('gltf-morph', {'value': 1})
        mountain.setAttribute('model-opacity', 0)
        mountain.removeAttribute('animation__height')
        mountain.removeAttribute('animation__alpha')
        setTimeout(() => {
          visible = true
        }, 50)
      }, 3000)
    }

    const openWindow = (e) => {
      window.open('https://advisory.kpmg.us/events/in-person-event-homepage/2023/kpmg-tech-innovation-symposium.html', '_blank')
    }

    this.el.sceneEl.addEventListener('xrimagefound', imageFound)
    this.el.sceneEl.addEventListener('xrimageupdated', showImage)
    this.el.sceneEl.addEventListener('xrimagelost', imageLost)
    enterEntity.addEventListener('click', openWindow)
    this.el.sceneEl.fog = new THREE.Fog(0xffffff, 50, 100)

    mountain.addEventListener('model-loaded', () => {
      mountain.getObject3D('mesh').traverse(node => {
        const {src} = document.getElementById('mountainTexture')
        if (node.name === 'DH_SM_MountainTop_Lowest' || node.name === 'Scene') {
          node.material.map = new THREE.TextureLoader().load(src)
          node.material.map.flipY = false
          node.material.needsUpdate = true
        }
      })
    })
  },
})
export {ARFunctions}