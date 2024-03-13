import React, {useState, useEffect} from "react";
import DataGrid, {
  Column,
  Grouping,
  GroupPanel,
  Pager,
  Paging,
  SearchPanel,
  Selection,
  Summary,
  TotalItem,
  HeaderFilter,
  FilterRow,
} from "devextreme-react/data-grid";
import LoadPanel from "devextreme-react/load-panel";

const Detail = ({data}) => {
    const [details, setDetails] = useState([]);

    function convertToUTCPlus7(localTime) {
        // Convert local time to UTC
        let utcTime = new Date(localTime);
        
        // Get the UTC time in milliseconds
        let utcMilliseconds = utcTime.getTime();
        
        // Calculate the UTC+7 time in milliseconds
        let utcPlus7Milliseconds = utcMilliseconds + (7 * 60 * 60 * 1000); // 7 hours in milliseconds
        
        // Create a new Date object for the UTC+7 time
        let utcPlus7Time = new Date(utcPlus7Milliseconds);
        
        return utcPlus7Time;
    }

    useEffect(() => {
        const formattedRecords = data.map((record) => ({
            ...record,
            created: convertToUTCPlus7(new Date(record.created)).toLocaleDateString(),
            arrivalTime: convertToUTCPlus7(new Date(record.arrivalTime)).toLocaleTimeString(),
            leaveTime: convertToUTCPlus7(new Date(record.leaveTime)).toLocaleTimeString(),
        }));
        setDetails(formattedRecords);
    }, []);
    
  return (
    <div>
      <DataGrid
        dataSource={details}
        showBorders={true}
        columnAutoWidth={true}
        noDataText="No users available"
        allowColumnResizing={true}
      >
        <HeaderFilter visible={true} />
        <Selection mode="single" />
        <GroupPanel visible={true} />
        <SearchPanel visible={true} highlightCaseSensitive={true} />
        <Grouping autoExpandAll={false} />
        <FilterRow visible={true} />

        <Column dataField="created" caption="Date" width="30%" />
        <Column dataField="arrivalTime" caption="Arrival Time" width="30%" />
        <Column dataField="leaveTime" caption="Leave Time" width="30%" />
        <Pager
          allowedPageSizes={[10, 25, 50, 100]}
          showPageSizeSelector={true}
        />
        <Paging defaultPageSize={10} />
        <Summary>
          <TotalItem column="email" summaryType="count" />
        </Summary>
      </DataGrid>
      <LoadPanel
        shadingColor="rgba(0,0,0,0.4)"
        visible={details === null}
        showIndicator={true}
        shading={true}
        position={{ of: "body" }}
      />
    </div>
  );
};

export { Detail };
