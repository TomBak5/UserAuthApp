export const logger = {
    error: (message: string, error: any) => {
        console.error('Error:', message);
        console.error('Details:', {
            message: error.message,
            status: error.response?.status,
            data: error.response?.data,
            stack: error.stack,
            timestamp: new Date().toISOString()
        });

        // You could also send errors to a logging service here
        // For now, we'll save to localStorage for debugging
        const logs = JSON.parse(localStorage.getItem('error_logs') || '[]');
        logs.push({
            message,
            error: {
                message: error.message,
                status: error.response?.status,
                data: error.response?.data,
                timestamp: new Date().toISOString()
            }
        });
        localStorage.setItem('error_logs', JSON.stringify(logs));
    },

    info: (message: string, data?: any) => {
        console.log('Info:', message);
        if (data) {
            console.log('Data:', data);
        }
    }
}; 