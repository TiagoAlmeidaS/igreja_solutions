import { faker } from '@faker-js/faker';
import type {
  LoginRequest,
  LoginResponse,
  User,
  HymnResponse,
  CreateHymnDto,
  Verse,
  VerseInput,
} from '../../src/types/api.js';
import type { HymnCategory } from '../../src/types/hymn.js';

/**
 * Gera dados fake para testes usando Faker
 */

export function createFakeUser(overrides?: Partial<User>): User {
  return {
    id: faker.string.uuid(),
    name: faker.person.fullName(),
    email: faker.internet.email(),
    ...overrides,
  };
}

export function createFakeLoginRequest(overrides?: Partial<LoginRequest>): LoginRequest {
  return {
    email: faker.internet.email(),
    password: faker.internet.password(),
    ...overrides,
  };
}

export function createFakeLoginResponse(overrides?: Partial<LoginResponse>): LoginResponse {
  return {
    token: faker.string.alphanumeric(100),
    user: createFakeUser(),
    ...overrides,
  };
}

export function createFakeVerse(overrides?: Partial<Verse>): Verse {
  return {
    type: faker.helpers.arrayElement(['V1', 'V2', 'V3', 'V4', 'R', 'Ponte', 'C']),
    lines: [
      faker.lorem.sentence(),
      faker.lorem.sentence(),
      faker.lorem.sentence(),
    ],
    ...overrides,
  };
}

export function createFakeVerseInput(overrides?: Partial<VerseInput>): VerseInput {
  return {
    type: faker.helpers.arrayElement(['V1', 'V2', 'V3', 'V4', 'R', 'Ponte', 'C']),
    lines: [
      faker.lorem.sentence(),
      faker.lorem.sentence(),
      faker.lorem.sentence(),
    ],
    ...overrides,
  };
}

export function createFakeHymnResponse(overrides?: Partial<HymnResponse>): HymnResponse {
  return {
    id: faker.number.int({ min: 1, max: 1000 }),
    number: faker.number.int({ min: 1, max: 999 }).toString(),
    title: faker.lorem.sentence(),
    category: faker.helpers.arrayElement(['hinario', 'canticos', 'suplementar', 'novos']) as HymnCategory,
    hymnBook: faker.lorem.words(3),
    key: faker.helpers.arrayElement(['C', 'D', 'E', 'F', 'G', 'A', 'B']),
    bpm: faker.number.int({ min: 60, max: 120 }),
    verses: [createFakeVerse(), createFakeVerse()],
    ...overrides,
  };
}

export function createFakeCreateHymnDto(overrides?: Partial<CreateHymnDto>): CreateHymnDto {
  return {
    number: faker.number.int({ min: 1, max: 999 }).toString(),
    title: faker.lorem.sentence(),
    category: faker.helpers.arrayElement(['hinario', 'canticos', 'suplementar', 'novos']) as HymnCategory,
    hymnBook: faker.lorem.words(3),
    key: faker.helpers.arrayElement(['C', 'D', 'E', 'F', 'G', 'A', 'B']),
    bpm: faker.number.int({ min: 60, max: 120 }),
    verses: [createFakeVerseInput(), createFakeVerseInput()],
    ...overrides,
  };
}

export function createFakeHymnList(count: number = 5): HymnResponse[] {
  return Array.from({ length: count }, () => createFakeHymnResponse());
}

