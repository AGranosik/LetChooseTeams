import { check, } from 'k6';
import { _basePostParams, _baseTournamentApiUrl } from '../variables.js';
import { postTournament } from './common.js';
export const options = {
    scenarios: {
        shared_scenario:{
            executor: 'shared-iterations',
            startTime: 0,
            vus: 50,
            iterations: 1000,
            maxDuration: '40s'
        },
        per_vu_iter_scernario:{
            executor: 'per-vu-iterations',
            startTime: '40s',
            vus: 50,
            iterations: 100,
            maxDuration: '25s'
        },
        cosntant_vu_scenario:{
            executor: 'constant-vus',
            startTime: '65s',
            vus: 100,
            duration: '20s',
        },
        ramping_vus_scenario:{
            executor: 'ramping-vus',
            startVUs: 0,
            startTime: '90s',
            stages: [
                { duration: '10s', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
                { duration: '20s', target: 200 }, // stay at 100 users for 10 minutes
                { duration: '30s', target: 0 }, // ramp-down to 0 users
            ],
            gracefulRampDown: '10s',
        }
    }
};


export default function () {
    const result = postTournament();
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}