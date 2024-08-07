import { Title, Stack, Center } from '@mantine/core';
import { useEffect, useState } from 'react';
import ErrorAlert from '../../common/ErrorAlert';
import BookList from './BookList';

const API_URL = "/api/books";

function Books() {
  const [error, setError] = useState(null);
  const [books, setBooks] = useState([]);

  // Fetch list of books
  useEffect(() => {
    fetchBooks();
  }, []);

  const fetchBooks = () => {
    setError('');
    fetch(`${API_URL}`)
      .then(response => {
        if (response.ok) {
          return response.json();
        }
        setError('Could not retrieve books'); // TODO, parse problem details
      })
      .then(json => setBooks(json))
      .catch (error => setError(error));
  }


  return (
    <Center>
      <Stack justify="center" w="95%" maw={900}>
        <Title order={2}>Books</Title>
        {error && <ErrorAlert message={error.message} />}
        <BookList books={books} />
      </Stack>
    </Center>
  );
}

export default Books;