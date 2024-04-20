const getters = {
  isMobile: state => state.app.isMobile,
  lang: state => state.app.lang,
  theme: state => state.app.theme,
  color: state => state.app.color,
  token: state => state.user.token,
  avatar: state => state.user.avatar,
  nickname: state => state.user.name,
  roles: state => state.user.roles,
  permissions: state => state.user.permissions,
  userInfo: state => state.user.info,
  addRouters: state => state.permission.addRouters,
  multiTab: state => state.app.multiTab,
  hasPermission: (state, getters) => (permissionCode) => {
    return getters.roles.some(x => x.name.toLowerCase() === 'admin') || getters.permissions.some(x => x.code.toLowerCase() === permissionCode.toLowerCase())
  }
}

export default getters
