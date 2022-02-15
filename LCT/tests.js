import http from 'k6/http';
import { check, sleep } from 'k6';
export const options = {
    vus: 10,
    duration: '10s',
    thresholds: {
        http_req_duration: ['p(99)<300'], // 99% of requests should be below 400ms
    },
};


export default function () {
    const payload = JSON.stringify({
        name: 'performance test',
        playerLimit: 2
    });

    const result = http.post(_baseApiUrl + '/create', payload, _basePostParams);
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
    sleep(1);
}