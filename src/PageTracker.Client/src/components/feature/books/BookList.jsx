import { Table, ActionIcon } from '@mantine/core';
import { Link } from 'react-router-dom';

function BookList({ books }) {

  const tableRows = books.map((book) => (
    <Table.Tr key={book.id}>
      <Table.Td>{book.title}</Table.Td>
      <Table.Td>{book.author}</Table.Td>
      <Table.Td>{book.startingPage}</Table.Td>
      <Table.Td>{book.endingPage}</Table.Td>
      <Table.Td maw="40">
        <ActionIcon variant="default" aria-label="Edit" component={Link} to={book.id.toString() + "/edit"}>
          <i className="fi fi-rs-pencil" />
        </ActionIcon>
        <ActionIcon variant="light" aria-label="Delete" color="red.4" ml="sm">
          <i className="fi fi-rs-trash" />
        </ActionIcon>
      </Table.Td>
    </Table.Tr>
  ));

  const tableHeader = (
    <Table.Tr>
      <Table.Th>Title</Table.Th>
      <Table.Th>Author</Table.Th>
      <Table.Th maw="50">Starting page</Table.Th>
      <Table.Th maw="50">Ending page</Table.Th>
      <Table.Th></Table.Th>
    </Table.Tr>
  );

  return (
    <Table highlightOnHover>
      <Table.Thead>{tableHeader}</Table.Thead>
      <Table.Tbody>{tableRows}</Table.Tbody>
    </Table>
  );
}

export default BookList;