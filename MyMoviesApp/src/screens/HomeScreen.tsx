import React, { useEffect, useState } from 'react';
import { View, Text, FlatList, TouchableOpacity, StyleSheet, ActivityIndicator } from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList, Theater } from '../models/types';
import { getTheaters } from '../api/mockApi';

type HomeScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Home'>;

type Props = {
  navigation: HomeScreenNavigationProp;
};

const HomeScreen: React.FC<Props> = ({ navigation }) => {
  const [theaters, setTheaters] = useState<Theater[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchTheaters = async () => {
      try {
        const data = await getTheaters();
        setTheaters(data);
      } catch (error) {
        console.error("Failed to load theaters", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTheaters();
  }, []);

  const renderTheater = ({ item }: { item: Theater }) => (
    <TouchableOpacity
      style={styles.theaterCard}
      onPress={() => navigation.navigate('Theater', { theaterCode: item.code })}
    >
      <Text style={styles.theaterName}>{item.name}</Text>
      <Text style={styles.movieCount}>{item.movies.length} Movies Playing</Text>
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#0000ff" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={theaters}
        keyExtractor={(item) => item.code}
        renderItem={renderTheater}
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
  theaterCard: {
    backgroundColor: '#fff',
    padding: 20,
    marginBottom: 16,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  theaterName: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  movieCount: {
    fontSize: 14,
    color: '#666',
  },
});

export default HomeScreen;
