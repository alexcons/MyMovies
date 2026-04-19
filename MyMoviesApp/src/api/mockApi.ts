import { Movie, Theater } from '../models/types';

// Mock Data
const MOVIES: Movie[] = [
  {
    code: 'm1',
    title: 'Dune: Part Two',
    synopsis: 'Paul Atreides unites with Chani and the Fremen while on a warpath of revenge against the conspirators who destroyed his family.',
    posterUri: 'https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2TGbiUU43.jpg',
    classification: 'PG-13',
    actors: 'Timothée Chalamet, Zendaya, Rebecca Ferguson',
    rating: '8.8/10',
    showtimes: [],
  },
  {
    code: 'm2',
    title: 'Kung Fu Panda 4',
    synopsis: 'After Po is tapped to become the Spiritual Leader of the Valley of Peace, he needs to find and train a new Dragon Warrior.',
    posterUri: 'https://image.tmdb.org/t/p/w500/kDp1vUBnMpe8ak4rjgl3cLELqjU.jpg',
    classification: 'PG',
    actors: 'Jack Black, Awkwafina, Viola Davis',
    rating: '7.5/10',
    showtimes: [],
  },
  {
    code: 'm3',
    title: 'Godzilla x Kong: The New Empire',
    synopsis: 'Two ancient titans, Godzilla and Kong, clash in an epic battle as humans unravel their intertwined origins and connection to Skull Island.',
    posterUri: 'https://image.tmdb.org/t/p/w500/tMefBSflR6PGQLvLuPEHZotffy.jpg',
    classification: 'PG-13',
    actors: 'Rebecca Hall, Brian Tyree Henry, Dan Stevens',
    rating: '7.1/10',
    showtimes: [],
  }
];

const THEATERS: Theater[] = [
  {
    code: 't1',
    name: 'Cinepolis Main Street',
    movies: [
      {
        ...MOVIES[0],
        showtimes: [
          { date: '14:30', movieCode: 'm1' },
          { date: '17:45', movieCode: 'm1' },
          { date: '21:00', movieCode: 'm1' }
        ]
      },
      {
        ...MOVIES[1],
        showtimes: [
          { date: '12:00', movieCode: 'm2' },
          { date: '14:15', movieCode: 'm2' },
          { date: '16:30', movieCode: 'm2' }
        ]
      }
    ]
  },
  {
    code: 't2',
    name: 'Cinemark Westend',
    movies: [
      {
        ...MOVIES[0],
        showtimes: [
          { date: '15:00', movieCode: 'm1' },
          { date: '18:30', movieCode: 'm1' },
          { date: '22:00', movieCode: 'm1' }
        ]
      },
      {
        ...MOVIES[2],
        showtimes: [
          { date: '13:00', movieCode: 'm3' },
          { date: '15:45', movieCode: 'm3' },
          { date: '18:30', movieCode: 'm3' },
          { date: '21:15', movieCode: 'm3' }
        ]
      }
    ]
  }
];

// Mock API Functions
export const getTheaters = async (): Promise<Theater[]> => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(THEATERS);
    }, 500); // Simulate network delay
  });
};

export const getTheater = async (code: string): Promise<Theater | undefined> => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(THEATERS.find(t => t.code === code));
    }, 500);
  });
};

export const getMovieInTheater = async (theaterCode: string, movieCode: string): Promise<Movie | undefined> => {
    return new Promise((resolve) => {
        setTimeout(() => {
            const theater = THEATERS.find(t => t.code === theaterCode);
            if (theater) {
                resolve(theater.movies.find(m => m.code === movieCode));
            } else {
                resolve(undefined);
            }
        }, 500);
    });
};
