export interface Showtime {
  date: string; // ISO 8601 string or simple time format
  movieCode: string;
}

export interface Movie {
  code: string;
  title: string;
  synopsis: string;
  posterUri: string;
  classification?: string;
  actors?: string;
  rating?: string;
  showtimes: Showtime[]; // Specific to the theater when viewed from TheaterScreen
}

export interface Theater {
  code: string;
  name: string;
  movies: Movie[]; // Nested for simplicity in mock, though original has separate showtimes list
}

export type RootStackParamList = {
  Home: undefined;
  Theater: { theaterCode: string };
  MovieDetail: { movieCode: string; theaterCode: string };
};
