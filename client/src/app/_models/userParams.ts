// why not the interface instead of class?
// Well, because we can init some values inside of the constructor

import { User } from "./user";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 40;
    pageNumber = 1;
    pageSize = 4;
    orderBy = 'lastActive';

    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}