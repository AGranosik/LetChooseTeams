import { check, sleep } from "k6";
import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';

// export const options = {
//     stages: [
//         { duration: '1m', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
//         { duration: '2m', target: 100 }, // stay at 100 users for 10 minutes
//         { duration: '1m', target: 0 }, // ramp-down to 0 users
//     ],
//     thresholds: {
//         http_req_duration: ['p(99)<200'], // 99% of requests should be below 400ms
//     },
// }

export const options = {
    vus: 5,
    duration: '10s'
};

  export default function () {
    const id = addTournamentToGet().split('"').join("");
    console.log(id);
    const url = new URL(_baseTournamentApiUrl);
    url.searchParams.append('Id', id);
    // const result = url.toString();
    const result = http.get(url.toString());
    console.log(JSON.stringify(result));
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}
function addTournamentToGet() {
    const rand = Math.floor((Math.random() * 10000) +1);
    const payload = JSON.stringify({
        name: 'performance test' + rand.toString(),
        playerLimit: rand
    });

    const result = http.post(_baseTournamentApiUrl + '/create', payload, _basePostParams);
    sleep(0.5);
    return result.body;
}