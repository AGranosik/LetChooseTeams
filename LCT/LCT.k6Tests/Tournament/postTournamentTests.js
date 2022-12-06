import { check, } from 'k6';
import { _basePostParams, _baseTournamentApiUrl } from '../variables.js';
import { postTournament } from './common.js';
export const options = {
    // 2s for request (post + qr code generation)
    scenarios: {
        per_vu_iter_scernario:{
            executor: 'per-vu-iterations',
            startTime: 0,
            vus: 100,
            iterations: 25,
            maxDuration: '55s'
        },
        shared_scenario:{
            executor: 'shared-iterations',
            startTime: '60s',
            vus: 100,
            iterations: 800,
            maxDuration: '40s'
        },
        cosntant_vu_scenario:{
            executor: 'constant-vus',
            startTime: '100s',
            vus: 100,
            duration: '35s',
        },
        ramping_vus_scenario:{
            executor: 'ramping-vus',
            startVUs: 0,
            startTime: '135s',
            stages: [
                { duration: '30s', target: 50 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
                { duration: '30s', target: 120 }, // stay at 100 users for 10 minutes
                { duration: '30s', target: 120 }, // stay at 100 users for 10 minutes
                { duration: '30s', target: 0 }, // ramp-down to 0 users
            ],
            gracefulRampDown: '15s',
        }
    },
    thresholds: {
        http_req_duration: ['p(90)<2000'],
    },
};


export default function () {
    const result = postTournament();
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}