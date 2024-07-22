function TodaysPagesHeading({ numPagesRead }) {

  const getHeading = (numPagesRead) => {
    if (numPagesRead <= 0) {
      return <h2>How many pages did you just read?</h2>;
    }
    else if (numPagesRead == 1) {
      return <h2 style={{ textAlign: "center" }}>One page is still progress!<br />Any more pages?</h2>
    }
    else {
      return <h2 style={{ textAlign: "center" }}>Nice! You&apos;ve read {numPagesRead} pages today<br />Any more pages?</h2>
    }
  };

  return (
    <div>{getHeading(numPagesRead)}</div>
  );
}

export default TodaysPagesHeading;