import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { RootStackParamList } from './src/models/types';

import HomeScreen from './src/screens/HomeScreen';
import TheaterScreen from './src/screens/TheaterScreen';
import MovieDetailScreen from './src/screens/MovieDetailScreen';

const Stack = createNativeStackNavigator<RootStackParamList>();

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="Home">
        <Stack.Screen
          name="Home"
          component={HomeScreen}
          options={{ title: 'MyMovies' }}
        />
        <Stack.Screen
          name="Theater"
          component={TheaterScreen}
          options={{ title: 'Theater' }}
        />
        <Stack.Screen
          name="MovieDetail"
          component={MovieDetailScreen}
          options={{ title: 'Movie Details' }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}
