import React, { useEffect, useState } from 'react';
import { View, Text, FlatList, TouchableOpacity, StyleSheet, ActivityIndicator, Image } from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList, Theater, Movie, Showtime } from '../models/types';
import { getTheater } from '../api/mockApi';

type TheaterScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Theater'>;
type TheaterScreenRouteProp = RouteProp<RootStackParamList, 'Theater'>;

type Props = {
  navigation: TheaterScreenNavigationProp;
  route: TheaterScreenRouteProp;
};

const TheaterScreen: React.FC<Props> = ({ navigation, route }) => {
  const { theaterCode } = route.params;
  const [theater, setTheater] = useState<Theater | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchTheater = async () => {
      try {
        const data = await getTheater(theaterCode);
        if (data) {
          setTheater(data);
          navigation.setOptions({ title: data.name });
        }
      } catch (error) {
        console.error("Failed to load theater", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTheater();
  }, [theaterCode, navigation]);

  const renderShowtimes = (showtimes: Showtime[]) => (
    <View style={styles.showtimesContainer}>
      {showtimes.map((st, index) => (
        <View key={index} style={styles.showtimeBadge}>
          <Text style={styles.showtimeText}>{st.date}</Text>
        </View>
      ))}
    </View>
  );

  const renderMovie = ({ item }: { item: Movie }) => (
    <TouchableOpacity
      style={styles.movieCard}
      onPress={() => navigation.navigate('MovieDetail', { movieCode: item.code, theaterCode })}
    >
      <Image source={{ uri: item.posterUri }} style={styles.poster} resizeMode="cover" />
      <View style={styles.movieInfo}>
        <Text style={styles.movieTitle}>{item.title}</Text>
        <Text style={styles.movieDetails} numberOfLines={2}>{item.synopsis}</Text>
        {renderShowtimes(item.showtimes)}
      </View>
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#0000ff" />
      </View>
    );
  }

  if (!theater) {
    return (
      <View style={styles.center}>
        <Text>Theater not found.</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={theater.movies}
        keyExtractor={(item) => item.code}
        renderItem={renderMovie}
        contentContainerStyle={styles.listContainer}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  listContainer: {
    padding: 16,
  },
  movieCard: {
    backgroundColor: '#fff',
    flexDirection: 'row',
    marginBottom: 16,
    borderRadius: 8,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  poster: {
    width: 100,
    height: 150,
  },
  movieInfo: {
    flex: 1,
    padding: 12,
  },
  movieTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  movieDetails: {
    fontSize: 12,
    color: '#666',
    marginBottom: 8,
  },
  showtimesContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: 'auto',
  },
  showtimeBadge: {
    backgroundColor: '#e0e0e0',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 4,
    marginRight: 6,
    marginBottom: 6,
  },
  showtimeText: {
    fontSize: 12,
    fontWeight: '500',
  },
});

export default TheaterScreen;
