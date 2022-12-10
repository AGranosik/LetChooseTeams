import { check, group, sleep } from "k6";
import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';
import { postTournament } from "./common.js";

export const options = {
    stages: [
        { duration: '30s', target: 50 },
        { duration: '30s', target: 100 }, 
        { duration: '30s', target: 100 },
        { duration: '40s', target: 0 },
    ],
    thresholds: {
        'group_duration{group:::get is ok}': ['avg < 2000'],
    },
}
  export default function () {
    var id;
    group('post is success', function(){
        const postResult = postTournament();
        id = postResult.body.split('"').join("");
    })
    const url = new URL(_baseTournamentApiUrl);
    url.searchParams.append('Id', id);
    group('get is ok', function(){
        const result = http.get(url.toString());
        check(result, {
            'is status 200:': (r) => r.status == 200
        });
    })
}