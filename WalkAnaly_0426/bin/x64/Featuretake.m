%%
clear all;

load('ACC_044butterworth.txt')
xdata=ACC_044butterworth;
clear ACC_044butterworth;

N=length(xdata);
figure,plot([1:N],xdata)
title('044 data')
%%
%butterworth
load('ACC_107_1.txt')
data=ACC_107_1;
clear ACC_107_1;
N=length(data);
figure,plot([1:N],data)
title('69-x ori data')
filterOrder=8;
[b, a] = butter(filterOrder, [12/(120/2)], 'low');
result = filter(b, a, data);
% 
% figure,plot([1:N],result)
% title('72-x data')
law=5/(110/2)
hi=15/(110/2)
[b, a] = butter(filterOrder, [law hi]);  
sresult = filter(b, a, data);
N=length(sresult);
figure,
subplot(2,1,1),plot([1:N],data)
title('ACC_107_1 ori data')
subplot(2,1,2),plot([1:N],sresult)
title('ACC_107_1 sresult')
%%
wave = diff(sresult);
    next = 0;
wavem=zeros(length(wave),1);
    for i=1:1:(length(wave)-2)
        if (next==0)
            if((wave(i)>0 && wave(i+1)<0) || (wave(i)<0 && wave(i+1)>0))   
                if(sresult(i+1)>0.5)
                wavem(i+1) = sresult(i+1);
                end
                next = 2;
            end
        else
            next = next-1;
        end
    end
N=length(wavem);
figure,plot([1:N],wavem)
title('69-wavem data')    
    
%%
load('ACC_073_1.txt')
data=ACC_073_1;
clear ACC_073_1;

sresult=spectrogram(data);
N=length(sresult);
% spectrogram(data,'yaxis')
figure,plot([1:N],sresult(1:N,7))
title('73-S data')
%%
load('ACC_105_1.txt')
data=ACC_105_1;
clear ACC_105_1;
N=length(data);
figure,plot([1:N],data)
title('69-x ori data')
filterOrder=8;
[b, a] = butter(filterOrder, [20/(120/2)], 'low');
result = filter(b, a, data);
% 
% figure,plot([1:N],result)
% title('72-x data')
law=5/(110/2)
hi=15/(110/2)
[b, a] = butter(filterOrder, [law hi]);  
sresult = filter(b, a, data);
N=length(sresult);
figure,
subplot(2,1,1),plot([1:N],data)
title('69-x ori data')
subplot(2,1,2),plot([1:N],result)
title('72-s result')