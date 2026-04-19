import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, Image, ScrollView } from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList, Movie } from '../models/types';
import { getMovieInTheater } from '../api/mockApi';

type MovieDetailScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'MovieDetail'>;
type MovieDetailScreenRouteProp = RouteProp<RootStackParamList, 'MovieDetail'>;

type Props = {
  navigation: MovieDetailScreenNavigationProp;
  route: MovieDetailScreenRouteProp;
};

const MovieDetailScreen: React.FC<Props> = ({ navigation, route }) => {
  const { movieCode, theaterCode } = route.params;
  const [movie, setMovie] = useState<Movie | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchMovie = async () => {
      try {
        const data = await getMovieInTheater(theaterCode, movieCode);
        if (data) {
          setMovie(data);
          navigation.setOptions({ title: data.title });
        }
      } catch (error) {
        console.error("Failed to load movie", error);
      } finally {
        setLoading(false);
      }
    };

    fetchMovie();
  }, [movieCode, theaterCode, navigation]);

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#0000ff" />
      </View>
    );
  }

  if (!movie) {
    return (
      <View style={styles.center}>
        <Text>Movie not found.</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Image source={{ uri: movie.posterUri }} style={styles.poster} resizeMode="cover" />
      <View style={styles.content}>
        <Text style={styles.title}>{movie.title}</Text>

        <View style={styles.metaContainer}>
          {movie.classification && (
            <Text style={styles.metaText}>Class: {movie.classification}</Text>
          )}
          {movie.rating && (
            <Text style={styles.metaText}>Rating: {movie.rating}</Text>
          )}
        </View>

        <Text style={styles.sectionTitle}>Synopsis</Text>
        <Text style={styles.bodyText}>{movie.synopsis}</Text>

        {movie.actors && (
          <>
            <Text style={styles.sectionTitle}>Actors</Text>
            <Text style={styles.bodyText}>{movie.actors}</Text>
          </>
        )}
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  poster: {
    width: '100%',
    height: 400,
  },
  content: {
    padding: 16,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  metaContainer: {
    flexDirection: 'row',
    marginBottom: 16,
  },
  metaText: {
    marginRight: 16,
    color: '#666',
    fontWeight: '500',
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginTop: 16,
    marginBottom: 8,
  },
  bodyText: {
    fontSize: 16,
    lineHeight: 24,
    color: '#333',
  },
});

export default MovieDetailScreen;
