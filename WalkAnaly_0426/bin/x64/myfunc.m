function [c, d] = myfunc(infilename, outfilename, filterOrder) 
data = load(infilename);
% ====== �گS�U���o�i�� ======
%[b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);  %�C�q1/(50/2)�A���q2/(50/2)
[b, a] = butter(filterOrder, [15/(120/2)], 'low');
y = filter(b, a, data);

c = data;
d = y;

fid=fopen(outfilename,'w');
fprintf(fid, '%.4f ', y);
fclose(fid);

