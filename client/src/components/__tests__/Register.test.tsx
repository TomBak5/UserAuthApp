import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import Register from '../Register';

describe('Register Component', () => {
    const renderComponent = () => {
        render(
            <BrowserRouter>
                <Register />
            </BrowserRouter>
        );
    };

    test('renders registration form', () => {
        renderComponent();
        
        expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    });

    test('validates required fields', async () => {
        renderComponent();
        
        const submitButton = screen.getByRole('button', { name: /register/i });
        fireEvent.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/first name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/last name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/email is required/i)).toBeInTheDocument();
            expect(screen.getByText(/password is required/i)).toBeInTheDocument();
        });
    });

    test('submits form with valid data', async () => {
        renderComponent();
        
        fireEvent.change(screen.getByLabelText(/first name/i), { target: { value: 'John' } });
        fireEvent.change(screen.getByLabelText(/last name/i), { target: { value: 'Doe' } });
        fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'john@example.com' } });
        fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'Password123!' } });

        const submitButton = screen.getByRole('button', { name: /register/i });
        fireEvent.click(submitButton);

        await waitFor(() => {
            expect(screen.queryByText(/is required/i)).not.toBeInTheDocument();
        });
    });
}); 