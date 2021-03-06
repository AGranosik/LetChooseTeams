import { check, group, sleep } from "k6";
import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';
import { postTournament } from "./common.js";

export const options = {
    stages: [
        { duration: '10s', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '20s', target: 200 }, // stay at 100 users for 10 minutes
        { duration: '30s', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(95)<1500'],
    },
}

  export default function () {
    var id;
    group('post is success', function(){
        const postResult = postTournament();
        id = postResult.body.split('"').join("");
        check(postResult, {
            'is status 200:': (r) => r.status == 200
        });
    })

    group('get is ok', function(){
        const url = new URL(_baseTournamentApiUrl);
        url.searchParams.append('Id', id);
        const result = http.get(url.toString());
        check(result, {
            'is status 200:': (r) => r.status == 200
        });
    })

}