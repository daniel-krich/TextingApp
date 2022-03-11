export class Utils {
    static ParseTimeOffset(currentTime: Date, time: Date): string{
        const timeOffsetSeconds = (currentTime.getTime() - time.getTime()) / 1000;
        if(timeOffsetSeconds >= 86400) { // days
            return `${Math.round(timeOffsetSeconds/86400)}d`;
        }
        else if(timeOffsetSeconds >= 3600) { // hours
            return `${Math.round(timeOffsetSeconds/3600)}h`;
        }
        else if(timeOffsetSeconds > 60) { // mins
            return `${Math.round(timeOffsetSeconds/60)}m`;
        }
        else if(timeOffsetSeconds >= 1) { // secs
            return `${Math.round(timeOffsetSeconds)}s`;
        }
        else {
            return "Now";
        }
    }
}