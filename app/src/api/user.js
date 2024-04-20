import request from '@/utils/request'

/**
 * login func
 * parameter: {
 *     username: '',
 *     password: '',
 *     remember_me: true,
 * }
 * @param parameter
 * @returns {*}
 */
export function login(parameter) {
  return request({
    url: '/user/auth',
    method: 'post',
    data: parameter
  })
}

export function getInfo() {
  return request({
    url: '/user/current',
    method: 'get'
  })
}

export function logout() {
  return request({
    url: '/user/logout',
    method: 'post'
  })
}

export function get(params) {
  return request({
    url: '/user',
    method: 'get',
    params: params
  })
}

export function add(data) {
  return request({
    url: '/user',
    method: 'post',
    data
  })
}

export function edit(data) {
  return request({
    url: '/user',
    method: 'put',
    data
  })
}

export function del(id) {
  return request({
      url: '/role?id=' + id,
      method: 'delete'
  })
}