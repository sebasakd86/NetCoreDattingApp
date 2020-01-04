import { Photo } from './photo';

export interface User {
    id: number; // LowerCase 4 TypeScript Properties
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interest?: string; // ? = Optional always go after the required
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
