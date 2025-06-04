import axios from 'axios';
import { LoginData, RegisterData, User } from '../types';
import { logger } from '../utils/logger';

const API_URL = 'http://localhost:5158/api/Auth';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Add request interceptor to include token in authenticated requests
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    logger.info('Making API request', {
        url: config.url,
        method: config.method,
        data: config.data
    });
    return config;
});

api.interceptors.response.use(
    response => {
        logger.info('API response received', {
            url: response.config.url,
            status: response.status,
            data: response.data
        });
        return response;
    },
    error => {
        logger.error('API request failed', error);
        throw error;
    }
);

export const register = async (data: RegisterData) => {
    try {
        console.log('Registering with data:', data);
        const response = await api.post('/register', data);
        console.log('Registration successful:', response.data);
        return response.data;
    } catch (error: any) {
        console.error('Registration failed:', error.response?.data || error.message);
        throw error;
    }
};

export const login = async (data: LoginData) => {
    try {
        const response = await api.post('/login', data);
        if (response.data.user && response.data.message === "Login successful") {
            return { token: response.data.user.token };
        }
        throw new Error(response.data.message || 'Login failed');
    } catch (error) {
        console.error('Login error:', error);
        throw error;
    }
};

export const getUsers = async (): Promise<User[]> => {
    try {
        const response = await api.get('/users');
        return response.data;
    } catch (error) {
        console.error('Get users error:', error);
        throw error;
    }
}; 