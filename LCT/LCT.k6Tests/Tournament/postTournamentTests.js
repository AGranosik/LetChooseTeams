import { check, } from 'k6';
import { _basePostParams, _baseTournamentApiUrl } from '../variables.js';
import { postTournament } from './common.js';
export const options = {
    scenarios: {
        ramping_vus_scenario:{
            executor: 'ramping-vus',
            startVUs: 0,
            startTime: 0,
            stages: [
                { duration: '30s', target: 50 },
                { duration: '30s', target: 120 },
                { duration: '30s', target: 80 },
                { duration: '30s', target: 0 },
            ],
            gracefulRampDown: '15s',
        },
        shared_scenario:{
            executor: 'shared-iterations',
            startTime: '130s',
            vus: 80,
            iterations: 1600,
            maxDuration: '40s'
        },
        per_vu_iter_scernario:{
            executor: 'per-vu-iterations',
            startTime: '175s',
            vus: 120,
            iterations: 25,
            maxDuration: '55s'
        },
        cosntant_vu_scenario:{
            executor: 'constant-vus',
            startTime: '235s',
            vus: 120,
            duration: '1m',
        }
    },
    thresholds: {
        http_req_duration: [' avg < 2000'],
    },
};


export default function () {
    const result = postTournament();
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}