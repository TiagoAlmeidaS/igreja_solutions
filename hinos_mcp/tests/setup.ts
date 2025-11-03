/**
 * Jest test setup
 */
import dotenv from 'dotenv';

// Load environment variables
dotenv.config({ path: '.env.test' });

// Mock console.error to reduce noise in tests
global.console = {
  ...console,
  error: jest.fn(),
};

