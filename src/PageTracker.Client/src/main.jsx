import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App';
import Books from './components/feature/books/Books';
import PageLog from './components/feature/pagelog/PageLog';
import { MantineProvider, createTheme } from '@mantine/core';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';

import '@mantine/core/styles.css';
import './assets/styles/uicons-regular-straight.css'

const theme = createTheme({
  colors: {
    'warm-red': ["#ffeaec", "#fdd4d6", "#f4a7ac", "#ec777e", "#e64f57", "#e3353f", "#e22732", "#c91a25", "#b31220", "#9e0419"],
    'teal-blue': ["#e4fdfd", "#d7f4f4", "#b3e5e5", "#8dd7d7", "#6dcbcb", "#57c3c3", "#49bfbf", "#37a8a8", "#269696", "#008282"]
  },
  scale: 1.25,
  black: "#333333",
  fontFamily: 'Zen Kaku Gothic New, sans-serif',
  headings: {
    fontWeight: 900,
    fontFamily: 'Berkshire Swash, serif',
  }
});

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        index: true,
        element: <PageLog />
      },
      {
        path: "/books",
        element: <Books />,
        children: [
          {
            path: "/books/:id",
            element: <Books />
          }
        ]
      }
    ]
  }
]);

ReactDOM.createRoot(document.getElementById('root')).render(
  <MantineProvider theme={theme} defaultColorScheme="light">
    <React.StrictMode>
      <RouterProvider router={router} />
    </React.StrictMode>
  </MantineProvider>,
)
