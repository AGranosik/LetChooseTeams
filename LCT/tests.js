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

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const result = http.post('https://localhost:7008/api/Tournament/create', payload, params);
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
    sleep(1);
}