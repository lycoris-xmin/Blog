import { ElLoading } from 'element-plus'

export default class loading {
  constructor(options) {
    Object.assign(this.options, options || {})
  }

  options = {
    lock: true,
    text: '',
    background: 'rgba(0, 0, 0, 0.7)',
    target: 'body'
  }

  _loading = void 0

  show() {
    this._loading = ElLoading.service(this.options)
  }

  hide() {
    this._loading.close()
  }
}
