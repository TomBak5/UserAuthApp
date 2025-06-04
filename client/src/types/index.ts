export interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber?: string;
    age?: number;
    country?: string;
    city?: string;
    address?: string;
}

export interface RegisterData {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    phoneNumber?: string;
    age?: number;
    country?: string;
    city?: string;
    address?: string;
}

export interface LoginData {
    email: string;
    password: string;
} 