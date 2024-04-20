import request from '@/utils/request'

export function get(params) {
  return request({
      url: '/role',
      method: 'get',
      params: params
  })
}

export function add(data) {
  return request({
      url: '/role',
      method: 'post',
      data
  })
}

export function edit(data) {
  return request({
      url: '/role',
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
