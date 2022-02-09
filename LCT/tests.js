import http from 'k6/http';
import { sleep } from 'k6';
export const options = {
    vus: 10,
    duration: '30s',
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

    http.post('https://localhost:7008/api/Tournament/create', payload, params);
    sleep(1);
}