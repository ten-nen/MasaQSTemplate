import request from '@/utils/request'

export function get(params) {
    return request({
        url: '/permission',
        method: 'get',
        params: params
    })
}

export function add(data) {
    return request({
        url: '/permission',
        method: 'post',
        data
    })
}

export function edit(data) {
    return request({
        url: '/permission',
        method: 'put',
        data
    })
}

export function del(id) {
    return request({
        url: '/permission?id=' + id,
        method: 'delete'
    })
}