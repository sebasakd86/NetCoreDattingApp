import { Photo } from './photo';

export interface User {
    id: number; // LowerCase 4 TypeScript Properties
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: any; // Date; // so we can avoid issues while going to production mode.
    photoUrl: string;
    city: string;
    country: string;
    interests?: string; // ? = Optional always go after the required
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
