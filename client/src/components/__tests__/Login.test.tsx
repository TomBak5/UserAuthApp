import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import '@testing-library/jest-dom';
import Login from '../Login';
import { AuthService } from '../../services/authService';

// Mock the auth service
jest.mock('../../services/authService');
const mockAuthService = AuthService as jest.Mocked<typeof AuthService>;

describe('Login Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    const renderLogin = () => {
        render(
            <BrowserRouter>
                <Login />
            </BrowserRouter>
        );
    };

    test('renders login form', () => {
        renderLogin();
        
        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
    });

    test('shows validation errors for empty fields', async () => {
        renderLogin();
        
        const loginButton = screen.getByRole('button', { name: /login/i });
        fireEvent.click(loginButton);

        await waitFor(() => {
            expect(screen.getByText(/email is required/i)).toBeInTheDocument();
            expect(screen.getByText(/password is required/i)).toBeInTheDocument();
        });
    });

    test('shows error message on failed login', async () => {
        mockAuthService.login.mockRejectedValue(new Error('Invalid credentials'));
        
        renderLogin();
        
        const emailInput = screen.getByLabelText(/email/i);
        const passwordInput = screen.getByLabelText(/password/i);
        const loginButton = screen.getByRole('button', { name: /login/i });

        fireEvent.change(emailInput, { target: { value: 'test@example.com' } });
        fireEvent.change(passwordInput, { target: { value: 'password123' } });
        fireEvent.click(loginButton);

        await waitFor(() => {
            expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument();
        });
    });

    test('successful login redirects user', async () => {
        mockAuthService.login.mockResolvedValue({ token: 'fake-token' });
        
        renderLogin();
        
        const emailInput = screen.getByLabelText(/email/i);
        const passwordInput = screen.getByLabelText(/password/i);
        const loginButton = screen.getByRole('button', { name: /login/i });

        fireEvent.change(emailInput, { target: { value: 'test@example.com' } });
        fireEvent.change(passwordInput, { target: { value: 'password123' } });
        fireEvent.click(loginButton);

        await waitFor(() => {
            expect(mockAuthService.login).toHaveBeenCalledWith({
                email: 'test@example.com',
                password: 'password123'
            });
        });
    });
}); 